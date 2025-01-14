import axios from 'axios';
import SimulationModel from '../models/SimulationModel';
import { API_URL } from './server_consts';



/**
 * Creates a new simulation from the given name and code.
 * @param name
 * @param code
 */
export async function createSimulation(name: string, code: string) {
    return await axios
        .put<SimulationModel>(`${API_URL}/simulations`, {
            name: name,
            participants: [],
            start_date: new Date().toISOString(),
            active: true,
            responses: JSON.stringify([]),
            asks: JSON.stringify([]),
            concessions: JSON.stringify([]),
            round: 0,
            code: code
        });
}

/**
 * Closes a simulation (non-deleting) by updating the active status and the end date.
 * @param code The code of the simulation to close.
 * @param endDate The date that the simulation was closed/ended. 
 * @returns
 */
export async function closeSimulation(code: string, endDate: Date = new Date()): Promise<any | undefined> {
    try {
        // Check for empty string
        if (code === "") {
            throw new Error("No code provided.");
        }

        return await axios
            .post(`${API_URL}/simulations/${code}/close`, {
                date: endDate.toISOString(),
            })
    } catch (error) {
        console.error(`Unable to close simulation: ${error}`);
        return undefined;
    }
}

export async function deleteSimulation(code: string): Promise<any | undefined> {
    try {
        // Check for empty string
        if (code === "") {
            throw new Error("No code provided.");
        }

        return await axios
            .delete(`${API_URL}/simulations/${code}`);
    } catch (error) {
        console.error(`Unable to delete simulation: ${error}`);
        return undefined;
    }
}

/**
 * Checks if a simulation exists.
 * @param code
 * @returns
 */
export async function doesSimulationExist(code: string): Promise<boolean> {
    try {
        const response: SimulationModel[] = await axios
            .get<SimulationModel[]>(`${API_URL}/simulations/${code}`)
            .then(response => response.data);

        return response.length > 0;

    } catch (error) {
        console.error(`Unable to check code: ${error}`);
        return true;
    }
}

/**
 * Generates a unique code for a simulation.
 * @param timeout Number of attempts to generate a unique code.
 * @returns
 */
export async function generateCode(timeout: number = 5): Promise<string> {
    for (let i = 0; i < timeout; i++) {
        const code = Math.random().toString(36).substring(7);
        const exists = await doesSimulationExist(code);
        if (!exists) return code;
    }
    throw new Error("Unable to generate unique code.");
}

/**
 * Queries a simulation from a code.
 * @param code
 * @returns
 */
export async function getSimulationFromCode(code: string): Promise<SimulationModel | undefined> {
    try {
        const response: SimulationModel = await axios
            .get<SimulationModel[]>(`${API_URL}/simulations/${code}`)
            .then(response => response.data)
            .then(data => {
                // Check if simulation exists
                if (data.length <= 0) {
                    throw new Error("Simulation not found.");
                }

                // Get first simulation
                return data[0];
            })
        return response;
    } catch (error) {
        console.error(`Unable to retrieve simulation from code '${code}': ${error}`);
        return undefined;
    }
}

/**
 * Queries all simulations.
 * @returns
 */
export async function getSimulations(): Promise<SimulationModel[] | undefined> {
    try {
        const response: SimulationModel[] = await axios
            .get(`${API_URL}/simulations`)
            .then<SimulationModel[]>((response) => response.data);
        return response;
    } catch (error) {
        console.error(`Unable to retrieve simulations: ${error}`);
        return undefined;
    }
}