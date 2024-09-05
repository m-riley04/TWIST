using System.Data.SqlClient;
using System.Data;
using TWISTServer.DatabaseComponents.Records;
using TWISTServer.Enums;

namespace TWISTServer.DatabaseComponents.DataAccessors
{
    public class ResponsesDataAccessor : DataAccessor
    {
        public IEnumerable<ResponseRecord> GetAllResponses()
        {
            string sql = $"select response_id, participant_id, simulation_id, survey_type, responses, submission_date from responses;";
            return Database.Query(
                sql,
                ResponseRecord.FromRow
            );
        }

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

        public IEnumerable<ResponseRecord> GetResponse(int responseId)
        {
            string sql = @"select 
response_id, participant_id, simulation_id, survey_type, responses, submission_date
FROM responses WHERE 
response_id = @response_id;";
            return Database.Query(
                sql,
                ResponseRecord.FromRow,
                [
                    new("@response_id", SqlDbType.Int) { Value = responseId },
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

        public int InsertResponse(ResponseRecord response)
        {
            string sql = @"
INSERT INTO responses 
(participant_id, simulation_id, survey_type, responses, submission_date) 
VALUES 
(@participant_id, @simulation_id, @survey_type, @responses, @submission_date)";

            SqlParameter[] parameters = [
                new("@participant_id", SqlDbType.Int) { Value = response.ParticipantId },
                new("@simulation_id", SqlDbType.Int) { Value = response.SimulationId },
                new("@survey_type", SqlDbType.Int) { Value = response.SurveyType },
                new("@responses", SqlDbType.NVarChar, -1) { Value = response.Responses},
                new("@submission_date", SqlDbType.DateTime) { Value = response.SubmissionDate},
            ];

            return Database.NonQuery(sql, parameters);
        }
    }
}
