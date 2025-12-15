using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace UniTrack.Application.Abstraction.Services.Logger
{
    public class MsSqlLogger : LoggerServiceBase
    {
        public MsSqlLogger(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var columnOptions = new ColumnOptions();

            // Standart sütunlardan 'Properties' XML/JSON karmaşasını temizlemek isterseniz:
            // columnOptions.Store.Remove(StandardColumn.Properties); 

            // Özel Sütunları Tanımlıyoruz (SQL Tablosuyla birebir aynı olmalı)
            columnOptions.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn { ColumnName = "Name", PropertyName = "Name", DataType = SqlDbType.NVarChar, DataLength = 255, AllowNull = true },
                new SqlColumn { ColumnName = "UserId", PropertyName = "UserId", DataType = SqlDbType.NVarChar, DataLength = 100, AllowNull = true },
                new SqlColumn { ColumnName = "ClientIp", PropertyName = "ClientIp", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = true },
                new SqlColumn { ColumnName = "Device", PropertyName = "Device", DataType = SqlDbType.NVarChar, DataLength = 500, AllowNull = true },
                new SqlColumn { ColumnName = "AuditStatus", PropertyName = "AuditStatus", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = true }
            };

            Logger = new LoggerConfiguration()
                .Enrich.FromLogContext() // Context'ten veri çekebilmek için kritik
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "Logs",
                        AutoCreateSqlTable = true 
                    },
                    columnOptions: columnOptions
                )
                .CreateLogger();
        }

    }
}
