import { useAuth0 } from "@auth0/auth0-react";
import LogoutButton from "../../components/LogoutButton";
import LoginButton from "../../components/LoginButton";

const InstructorPage = () => {
    const { isAuthenticated } = useAuth0();

    // Authenticated view
    if (isAuthenticated) return ( 
        <>
            <h1>Instructor Home</h1>
            <p>You can control, view, and create simulations here.</p>
            <LogoutButton />
        </>
    );

    // Fallback login
    return (
        <>
            <h1>Instructor Login</h1>
            <p>Click the button below to be redirected to the instructor login screen.</p>
            <LoginButton />
        </>
    );
}

export default InstructorPage;