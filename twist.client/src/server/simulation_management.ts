import axios from 'axios';
import SimulationModel from '../models/SimulationModel';
import { API_URL } from './server_consts';
import SimulationStateEnum from '../enums/SimulationStateEnum';
import AskModel from '../models/AskModel';
import CountryEnum from '../enums/CountryEnum';
import ConcessionModel from '../models/ConcessionModel';
import DefaultAskModel from '../models/DefaultAskModel';
import DefaultConcessionModel from '../models/DefaultConcessionModel';
import AgreementModel from '../models/AgreementModel';

/// Backup list of default asks for the USA
const DEFAULT_USA_ASKS = [
    "China to issue an improved nationwide negative list for foreign investment (especially take measures to liberalize the financial sector)."
    , "China to remove or reduce restrictions on foreign investment identified by the U.S."
    , "China to eliminate laws and regulations, such as licensing or procurement, that treat foreign entities less favorably than domestic Chinese firms."
    , "China to remove specified non-tariff barriers on US imports (ex. Onerous permitting, excessive inspections etc)"
    , "China to recognize that the U.S. may impose import quotas and tariffs on products in critical sectors."
    , "China to eliminate specific policies and practices linked to forced technology transfer."
    , "China to strengthen intellectual property protection."
    , "China to increase import of American agricultural products by $30-40 billion."
    , "China to increase import of American energy products by $20-30 billion."
    , "China to commit to the reduction of the trade deficit between China and the U.S.by $100-200 billion by 2020."
    , "China to cease government-sponsored or tolerated cyber espionage and intrusions into U.S. commercial networks."
    , "China to cease subsidies and other forms of assistance that support industries targeted in the MiC 2025 plan and other emerging and strategic industries."
    , "China to establish a high-level dialogue with U.S. to discuss dual-use technologies."
    , "China to refrain from military development of man-made islands in the South China Sea."
    , "China to help identify and discourage Chinese firms that evade U.S. sanctions against Iran and North Korea."
    , "China to guarantee human rights and democracy in Hong Kong"
]

/// Backup list of default asks for the PRC
const DEFAULT_PRC_ASKS = [
    "The U.S. to reduce tariffs on Chinese imports to 2017 levels."
    , "The U.S. to lift bans on high technology exports such as integrated circuits and aircraft to China."
    , "The U.S. to agree to a more limited approach in defining its export control regime."
    , "The U.S. to  remove Chinese companies like Huawei from the entities list."
    , "The U.S. to give equal treatment to Chinese companies in national security review (CFIUS)."
    , "The U.S. to open government procurement to Chinese technology products and services."
    , "The U.S. to refrain from restricting visas for Chinese students and professionals."
    , "The U.S. to recognize core Chinese national interests: keeping national unity of mainland China and Tibet, Xinjiang, Hong Kong, and Taiwan."
    , "The U.S. to recognize core Chinese national interests: sovereignty over South China Sea."
    , "The U.S. to agree not to send warships or military personnel to Taiwan."
]

/// Backup list of default concessions for the USA
const DEFAULT_USA_CONCESSIONS = [
    "The U.S. to reduce tariffs on Chinese imports to 2017 levels."
    , "The U.S. to lift bans on high technology exports such as integrated circuits and aircraft to China."
    , "The U.S. to agree to a more limited approach in defining its export control regime."
    , "The U.S. to  remove Chinese companies like Huawei from the entities list."
    , "The U.S. to give equal treatment to Chinese companies in national security review (CFIUS)."
    , "The U.S. to open government procurement to Chinese technology products and services."
    , "The U.S. to refrain from restricting visas for Chinese students and professionals."
    , "The U.S. to recognize core Chinese national interests: keeping national unity of mainland China and Tibet, Xinjiang, Hong Kong, and Taiwan."
    , "The U.S. to recognize core Chinese national interests: sovereignty over South China Sea."
    , "The U.S. to agree not to send warships or military personnel to Taiwan."
]

/// Backup list of default concessions for the PRC
const DEFAULT_PRC_CONCESSIONS = [
    "China to issue an improved nationwide negative list for foreign investment (especially take measures to liberalize the financial sector)."
    , "China to remove or reduce restrictions on foreign investment identified by the U.S."
    , "China to eliminate laws and regulations, such as licensing or procurement, that treat foreign entities less favorably than domestic Chinese firms."
    , "China to remove specified non-tariff barriers on US imports (ex. Onerous permitting, excessive inspections etc)"
    , "China to recognize that the U.S. may impose import quotas and tariffs on products in critical sectors."
    , "China to eliminate specific policies and practices linked to forced technology transfer."
    , "China to strengthen intellectual property protection."
    , "China to increase import of American agricultural products by $30-40 billion."
    , "China to increase import of American energy products by $20-30 billion."
    , "China to commit to the reduction of the trade deficit between China and the U.S.by $100-200 billion by 2020."
    , "China to cease government-sponsored or tolerated cyber espionage and intrusions into U.S. commercial networks."
    , "China to cease subsidies and other forms of assistance that support industries targeted in the MiC 2025 plan and other emerging and strategic industries."
    , "China to establish a high-level dialogue with U.S. to discuss dual-use technologies."
    , "China to refrain from military development of man-made islands in the South China Sea."
    , "China to help identify and discourage Chinese firms that evade U.S. sanctions against Iran and North Korea."
    , "China to guarantee human rights and democracy in Hong Kong"
]

/**
 * Creates a new simulation from the given name and code.
 * @param name
 * @param code
 */
export async function createSimulation(name: string, code: string) {
    return await axios
        .put<SimulationModel>(`${API_URL}/simulations`, {
            name: name,
            code: code,
            start_date: new Date().toISOString(),
            modified_date: new Date().toISOString(),
            round: 0,
            active: true,
            state: SimulationStateEnum.OPENED,
            instructor_id: 0 /// TODO: Actually put instructor_id when we have it
        });
}

/**
 * Retrieves the default/template asks for a given country.
 * @param country
 * @returns
 */
export async function getDefaultAsksByCountry(country: CountryEnum) {
    return await axios
        .get(`${API_URL}/defaults/asks/${country}`)
        .then<DefaultAskModel[]>(result => result.data)
}

/**
 * Retrieves the default/template concessions for a given country.
 * @param country
 * @returns
 */
export async function getDefaultConcessionsByCountry(country: CountryEnum) {
    return await axios
        .get(`${API_URL}/defaults/concessions/${country}`)
        .then<DefaultConcessionModel[]>(result => result.data)
}

export async function createSimulationAsksByCountry(simulationId: number, country: CountryEnum) {
    // Get the default asks
    const defaultAsks = await getDefaultAsksByCountry(country)
        //.catch(reason => console.error(`Unable to create simulation asks: ${reason}`));

    // Create the asks
    return await axios
        .put<AskModel[]>(`${API_URL}/asks-concessions/asks/batch`, defaultAsks.map((ask, i) => ({
            ask_id: i, // Doesn't actually set the id; done automatically by db
            simulation_id: simulationId,
            description: ask.description,
            points: 0,
            status: 0,
            creation_date: new Date(),
            modified_date: new Date(),
            country: country,
        })));
}

export async function createSimulationConcessionsByCountry(simulationId: number, country: CountryEnum) {

    // Get default concessions
    const defaultConcessions = await getDefaultConcessionsByCountry(country)

    // Create the concessions
    return await axios
        .put<ConcessionModel[]>(`${API_URL}/asks-concessions/concessions/batch`, defaultConcessions.map((concession, i) => ({
            concession_id: i,
            simulation_id: simulationId,
            description: concession.description,
            points: 0,
            status: 0,
            creation_date: new Date(),
            modified_date: new Date(),
            country: country,
        })));
}

export async function getAsksBySimAndCountry(simulationId: number, country: CountryEnum): Promise<AskModel[] | undefined> {
    return await axios
        .get<AskModel[]>(`${API_URL}/asks-concessions/asks/${simulationId}-${country}`)
        .then(response => response.data)
}

export async function getConcessionsBySimAndCountry(simulationId: number, country: CountryEnum): Promise<ConcessionModel[] | undefined> {
    return await axios
        .get<ConcessionModel[]>(`${API_URL}/asks-concessions/concessions/${simulationId}-${country}`)
        .then(response => response.data)
}

export async function getAgreementsBySim(simulationId: number): Promise<AgreementModel[] | undefined> {
    return await axios
        .get<AgreementModel[]>(`${API_URL}/agreements/${simulationId}`)
        .then(response => response.data)
}

/**
 * Closes a simulation (non-deleting) by updating the active status and the end date.
 * @param code The code of the simulation to close.
 * @param endDate The date that the simulation was closed/ended. 
 * @returns
 */
export async function closeSimulation(code: string, endDate: Date = new Date()) {
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

export async function deleteSimulationAsks(simulationId: number) {
    return await axios
        .delete(`${API_URL}/asks-concessions/asks/${simulationId}`)
}

export async function deleteSimulationConcessions(simulationId: number) {
    return await axios
        .delete(`${API_URL}/asks-concessions/concessions/${simulationId}`)
}

export async function deleteSimulation(code: string, simulationId: number) {
    try {
        // Check for empty string
        if (code === "") {
            throw new Error("No code provided.");
        }

        // Delete the simulation's asks
        deleteSimulationAsks(simulationId)

        // Delete the simulation's concessions
        deleteSimulationConcessions(simulationId)

        /// TODO: Delete the simulation's joint agreements

        // Delete the simulation
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