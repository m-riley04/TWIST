using System.Data;
using TWISTServer.Enums;
using TWISTServer.Interfaces;

namespace TWISTServer.DatabaseComponents.Records
{
    public record ResponseRecord(int ResponseId, int ParticipantId, int SimulationId, 
        SurveyTypeEnum SurveyType, string Responses, DateTime SubmissionDate) : IDatabaseRecord<ResponseRecord>
    {
        public static Dictionary<string, SqlDbType> Columns { get; } = new Dictionary<string, SqlDbType>()
        {
            { "response_id", SqlDbType.Int },
            { "participant_id", SqlDbType.Int },
            { "simulation_id", SqlDbType.Int },
            { "survey_type", SqlDbType.Int },
            { "responses", SqlDbType.NVarChar },
            { "submission_date", SqlDbType.DateTime },
        };

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
