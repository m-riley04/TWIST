import axios from 'axios';
import SimulationModel from '../models/SimulationModel';

const API_URL = "https://localhost:7026/api";

/**
 * Creates a new simulation from the given name and code.
 * @param name
 * @param code
 */
export async function createSimulation(name: string, code: string): Promise<any | undefined> {
    try {
        return await axios.put<SimulationModel>(`${API_URL}/simulations`, {
            name: name,
            participants: JSON.stringify([]),
            start_date: new Date().toISOString(),
            end_date: new Date().toISOString(),
            active: false,
            responses: JSON.stringify([]),
            asks: JSON.stringify([]),
            concessions: JSON.stringify([]),
            round: 0,
            code: code
        });
    } catch (error) {
        console.log(`Unable to create simulation: ${error}`);
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
            .then((response) => response.data);
        return response;
    } catch (error) {
        console.error(`Unable to retrieve simulations: ${error}`);
        return undefined;
    }
}