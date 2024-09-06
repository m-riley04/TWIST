using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;
using TWISTServer.Enums;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class ResponsesDataAccessor : DataAccessor<ResponseRecord>
    {
        public override string PrimaryKeyColumn => "response_id";
        public override string TableName => "responses";

        public IEnumerable<ResponseRecord> GetResponsesByType(SurveyTypeEnum type)
        {
            string sql = @"select 
response_id, participant_id, simulation_id, survey_type, responses, submission_date 
FROM responses
WHERE survey_type = @survey_type;";
            return Database.Query(
                sql,
                ResponseRecord.FromRow,
                [
                    new("@survey_type", SqlDbType.Int) { Value = type },
                ]
            );
        }

        public IEnumerable<ResponseRecord> GetResponseBySimulationId(int simulationId)
        {
            string sql = @"select 
response_id, participant_id, simulation_id, survey_type, responses, submission_date
FROM responses WHERE 
simulation_id = @simulation_id;";
            return Database.Query(
                sql,
                ResponseRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                ]
            );
        }

        public IEnumerable<ResponseRecord> GetResponseBySimulationIdAndSurveyType(int simulationId, SurveyTypeEnum surveyType)
        {
            string sql = @"select 
response_id, participant_id, simulation_id, survey_type, responses, submission_date
FROM responses WHERE 
simulation_id = @simulation_id AND
survey_type = @survey_type;";
            return Database.Query(
                sql,
                ResponseRecord.FromRow,
                [
                    new("@simulation_id", SqlDbType.Int) { Value = simulationId },
                    new("@survey_type", SqlDbType.Int) { Value = surveyType },
                ]
            );
        }
    }
}
