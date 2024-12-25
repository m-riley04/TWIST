import { useAuth0 } from "@auth0/auth0-react";
import LogoutButton from "../../components/LogoutButton";

const InstructorPage = () => {
    const { isAuthenticated, error, isLoading, loginWithRedirect, user, getAccessTokenSilently } = useAuth0();

    if (error) return <div>Oops... {error.message}</div>;

    if (isLoading) return <div>Loading...</div>;

    // Authenticated view
    if (isAuthenticated) return ( 
        <>
            <h1>Instructor Home</h1>
            <p><i>Account: {user?.name}</i></p>
            <p>You can control, view, and create simulations here.</p>
            <LogoutButton />
        </>
    );

    // Fallback login
    loginWithRedirect();
}

export default InstructorPage;