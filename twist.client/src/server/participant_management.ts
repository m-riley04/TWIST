import axios from "axios";
import ParticipantModel from "../models/ParticipantModel";
import { API_URL } from "./server_consts";

/**
 * Gets all the participants in a simulation.
 */
export async function getParticipants(simulationId: number): Promise<ParticipantModel[]> {
    try {
        return await axios
            .get(`${API_URL}/participants/simulation/${simulationId}`)
            .then<ParticipantModel[]>((response) => response.data);
    } catch (error) {
        console.error(`Unable to retrieve participants: ${error}`);
        return [];
    }
}