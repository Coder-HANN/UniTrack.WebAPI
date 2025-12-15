using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniTrack.Application.Common;
using UniTrack.Application.DTOs.Profile;

namespace UniTrack.Application.Feature.Profile.Query
{
    public class GetAllUserQuery : IRequest<ServiceResponse<List<GetAllUserQueryResponseDTO>>>
    {
        public GetAllUserQuery() { }
    }
}
