import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Rate, Trend, Counter } from 'k6/metrics';

// ─── Özel Metrikler ────────────────────────────────────────────────
const errorRate = new Rate('error_rate');
const loginDuration = new Trend('login_duration');
const eventsDuration = new Trend('events_duration');
const clubsDuration = new Trend('clubs_duration');
const requestCount = new Counter('total_requests');

// ─── Test Konfigürasyonu ────────────────────────────────────────────
export const options = {
    scenarios: {
        // Senaryo 1: Smoke Test (sistem ayakta mı?)
        smoke_test: {
            executor: 'constant-vus',
            vus: 2,
            duration: '30s',
            tags: { scenario: 'smoke' },
        },
        // Senaryo 2: Load Test (normal yük)
        load_test: {
            executor: 'ramping-vus',
            startVUs: 0,
            stages: [
                { duration: '1m', target: 20 },   // Ramp-up
                { duration: '3m', target: 20 },   // Sustained load
                { duration: '30s', target: 0 },   // Ramp-down
            ],
            startTime: '35s',
            tags: { scenario: 'load' },
        },
        // Senaryo 3: Stress Test (sistem sınırı)
        stress_test: {
            executor: 'ramping-vus',
            startVUs: 0,
            stages: [
                { duration: '1m', target: 50 },
                { duration: '2m', target: 100 },
                { duration: '1m', target: 0 },
            ],
            startTime: '5m35s',
            tags: { scenario: 'stress' },
        },
    },
    thresholds: {
        // P95 yanıt süresi 500ms altında olmalı
        'http_req_duration': ['p(95)<500'],
        // Hata oranı %5'in altında kalmalı
        'error_rate': ['rate<0.05'],
        // Login P95 < 300ms
        'login_duration': ['p(95)<300'],
        // Events P95 < 400ms
        'events_duration': ['p(95)<400'],
    },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5236';

// ─── Test Verisi ────────────────────────────────────────────────────
const TEST_USER = {
    email: __ENV.TEST_EMAIL || 'b@stu.rumeli.edu.tr',
    password: __ENV.TEST_PASSWORD || '123456',
};

// ─── Yardımcı: Cookie'den Token Al ─────────────────────────────────
function getCookieToken(res) {
    const setCookie = res.headers['Set-Cookie'] || '';
    const match = setCookie.match(/auth_token=([^;]+)/);
    return match ? match[1] : null;
}

// ─── Ana Test Fonksiyonu ────────────────────────────────────────────
export default function () {
    let authCookies = {};

    // ── 1. LOGIN ───────────────────────────────────────────────────────
    group('Authentication', () => {
        const loginPayload = JSON.stringify(TEST_USER);
        const loginParams = {
            headers: { 'Content-Type': 'application/json' },
            tags: { endpoint: 'POST /api/public/Login/Login' },
        };

        const start = Date.now();
        const loginRes = http.post(
            `${BASE_URL}/api/public/Login/Login`,
            loginPayload,
            loginParams
        );
        loginDuration.add(Date.now() - start);
        requestCount.add(1);

        const loginOk = check(loginRes, {
            'login status 200': r => r.status === 200,
            'login isSuccess true': r => {
                try { return JSON.parse(r.body).isSuccess === true; } catch { return false; }
            },
            'auth_token cookie set': r => r.headers['Set-Cookie']?.includes('auth_token') ?? false,
        });

        errorRate.add(!loginOk);

        if (loginRes.cookies['auth_token']) {
            authCookies = { Cookie: `auth_token=${loginRes.cookies['auth_token'][0].value}` };
        }
    });

    sleep(0.5);

    // ── 2. ETKİNLİKLER ────────────────────────────────────────────────
    group('Events Endpoints', () => {
        const params = { headers: authCookies, tags: { endpoint: 'GET /api/Event' } };

        const start = Date.now();
        const eventsRes = http.get(`${BASE_URL}/api/Event?pageIndex=0&pageSize=10`, params);
        eventsDuration.add(Date.now() - start);
        requestCount.add(1);

        check(eventsRes, {
            'events status 200': r => r.status === 200,
            'events has data': r => {
                try { return Array.isArray(JSON.parse(r.body).data); } catch { return false; }
            },
        });
        errorRate.add(eventsRes.status !== 200);

        sleep(0.3);

        // Tekil etkinlik
        const singleStart = Date.now();
        const singleParams = { headers: authCookies, tags: { endpoint: 'GET /api/Event/{id}' } };
        const singleRes = http.get(`${BASE_URL}/api/Event/1`, singleParams);
        eventsDuration.add(Date.now() - singleStart);
        requestCount.add(1);

        check(singleRes, {
            'single event status 200 or 404': r => [200, 404].includes(r.status),
        });
    });

    sleep(0.3);

    // ── 3. KULÜPLER ───────────────────────────────────────────────────
    group('Clubs Endpoints', () => {
        const params = { headers: authCookies, tags: { endpoint: 'GET /api/Club' } };

        const start = Date.now();
        const clubsRes = http.get(`${BASE_URL}/api/Club?pageIndex=0&pageSize=10`, params);
        clubsDuration.add(Date.now() - start);
        requestCount.add(1);

        check(clubsRes, {
            'clubs status 200': r => r.status === 200,
        });
        errorRate.add(clubsRes.status !== 200);
    });

    sleep(0.3);

    // ── 4. BİLDİRİMLER ───────────────────────────────────────────────
    group('Notifications', () => {
        const params = { headers: authCookies, tags: { endpoint: 'GET /api/Notification' } };
        const res = http.get(`${BASE_URL}/api/Notification?pageIndex=0&pageSize=10`, params);
        requestCount.add(1);

        check(res, {
            'notifications status 200 or 401': r => [200, 401].includes(r.status),
        });
    });

    sleep(1);
}

// ─── Özet Rapor ────────────────────────────────────────────────────
export function handleSummary(data) {
    return {
        'k6-results-summary.json': JSON.stringify(data, null, 2),
        stdout: textSummary(data, { indent: ' ', enableColors: true }),
    };
}

// k6 built-in summary helper
function textSummary(data, opts) {
    return `
═══════════════════════════════════════════════════
  UniTrack API — k6 Yük Testi Özet Raporu
═══════════════════════════════════════════════════
  Toplam İstek    : ${data.metrics.total_requests?.values?.count ?? 'N/A'}
  Hata Oranı      : ${((data.metrics.error_rate?.values?.rate ?? 0) * 100).toFixed(2)}%
  Login P95       : ${data.metrics.login_duration?.values?.['p(95)']?.toFixed(2) ?? 'N/A'} ms
  Events P95      : ${data.metrics.events_duration?.values?.['p(95)']?.toFixed(2) ?? 'N/A'} ms
  Clubs P95       : ${data.metrics.clubs_duration?.values?.['p(95)']?.toFixed(2) ?? 'N/A'} ms
  Genel P95       : ${data.metrics.http_req_duration?.values?.['p(95)']?.toFixed(2) ?? 'N/A'} ms
  Genel P99       : ${data.metrics.http_req_duration?.values?.['p(99)']?.toFixed(2) ?? 'N/A'} ms
═══════════════════════════════════════════════════
`;
}