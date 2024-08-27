using System.Data;
using TWISTServer.Enums;

namespace TWISTServer.Database.Records
{
    public record ResponseRecord(int ResponseId, int ParticipantId, int SimulationId, SurveyTypeEnum SurveyType, string Responses, DateTime SubmissionDate)
    {
        public static ResponseRecord FromRow(DataRow row)
        {
            return new ResponseRecord(
                row.Field<int>("response_id")
                , row.Field<int>("participant_id")
                , row.Field<int>("simulation_id")
                , (SurveyTypeEnum)row.Field<int>("survey_type")
                , row.Field<string>("responses") ?? ""
                , row.Field<DateTime>("submission_date")
                );
        }
    }
}
