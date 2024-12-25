const ParticipantLoginPage = () => {

    return (
        <>
            <h1>KU Trade War Simulation</h1>
            <p>Enter the room code to begin.</p>
            <form>
                <label htmlFor="text">Room Code:</label><br />
                <input id="code" title="Code" type="text" placeholder="Enter your code here..." /><br />
                <button type="submit">Join</button>
            </form>

            <a href="/instructor">Instructor Login</a>
        </>
    );
}

export default ParticipantLoginPage;