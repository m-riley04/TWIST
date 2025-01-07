import { useAuth0 } from "@auth0/auth0-react";
import LogoutButton from "../../components/LogoutButton";
import SimulationsList from "../../components/SimulationList/SimulationList";
import { useState } from "react";
import { useEffect } from "react";
import { Button, Modal } from "react-bootstrap";
import { deleteSimulation, getSimulations } from "../../server/simulation_management";
import SimulationModel from "../../models/SimulationModel";

const InstructorPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect, user } = useAuth0();
    const [simulations, setSimulations] = useState<SimulationModel[]>([]);
    const [show, setShow] = useState(false);
    const [selectedSimulation, setSelectedSimulation] = useState<SimulationModel>();
    const [selectedIndex, setSelectedIndex] = useState<number>();

    const handleClose = () => setShow(false);
    const handleDeleteClicked = (sim: SimulationModel, i: number) => {
        // Show modal
        setShow(true);

        // Get the selected simulation and index
        setSelectedSimulation(sim);
        setSelectedIndex(i);
    }
    const handleSimulationDelete = () => {
        // Check if the selected simulation is undefined
        if (selectedSimulation === undefined) {
            console.error("Unable to delete simulation: No simulation selected.");
            return;
        }

        // Delete simulation
        deleteSimulation(selectedSimulation.code)
            .then(() => {
                // Remove simulation from list
                setSimulations(simulations.filter((sim, i) => i !== selectedIndex));
            });

        // Close modal
        handleClose();
    }
    
    useEffect(() => {
        // Load simulations data from API
        getSimulations()
            .then(data => {
                if (data !== undefined) {
                    setSimulations(data);
                }
            });
    }, []);

    if (error) return <div>Oops... {error.message}</div>;

    if (isLoading) return <div>Loading...</div>;

    // Authenticated view
    if (isAuthenticated) return ( 
        <>
            <Modal show={show} onHide={handleClose}>
                <Modal.Header>
                    <Modal.Title>Confirm deletion.</Modal.Title>
                </Modal.Header>

                <Modal.Body>Are you sure you want to remove this simulation?</Modal.Body>

                <Modal.Footer>
                    <Button variant="danger" onClick={handleSimulationDelete}>Delete</Button>
                    <Button variant="primary" onClick={handleClose}>Cancel</Button>
                </Modal.Footer>

            </Modal>

            <h1>Instructor Home</h1>
            <p><i>Account: {user?.name}</i></p>
            <p>You can control, view, and create simulations here.</p>
            <h2>Simulations</h2>
            <SimulationsList simulations={simulations} onDeleteClicked={handleDeleteClicked} />
            <Button onClick={() => { window.location.assign("/instructor/create") }}>Create New Simulation</Button>
            <LogoutButton />
            <a href="/">Participant Home</a>
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default InstructorPage;