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

/**
 * Gets a participant by their email.
 */
export async function getParticipantByEmail(email: string): Promise<ParticipantModel[]> {
    return await axios
        .get(`${API_URL}/participants/email/${email}`)
        .then<ParticipantModel[]>((response) => response.data);
}

/**
 * Gets a participant by their email and simulation.
 */
export async function getParticipantByEmailAndSimulation(email: string, simulationId: number): Promise<ParticipantModel[]> {
    return await axios
        .get(`${API_URL}/participants/email-sim/${email}-${simulationId}`)
        .then<ParticipantModel[]>((response) => response.data);
}