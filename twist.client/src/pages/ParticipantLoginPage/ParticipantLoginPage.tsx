import { Button, Form, FormLabel } from "react-bootstrap";

const ParticipantLoginPage = () => {

    return (
        <>
            <h1>KU Trade War Simulation</h1>
            <p>Enter the room code to begin.</p>
            <Form>
                <Form.Group>
                    <Form.Label htmlFor="text">Room Code:</Form.Label><br />
                    <Form.Control id="code" title="Code" type="text" placeholder="Enter your code here..." />
                    <Button type="submit">Join</Button>
                </Form.Group>
            </Form>

            <a href="/instructor">Instructor Login</a>
        </>
    );
}

export default ParticipantLoginPage;