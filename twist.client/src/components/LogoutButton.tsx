import { useAuth0 } from "@auth0/auth0-react";

const LogoutButton = () => {
    const { logout, isAuthenticated } = useAuth0();

    if (isAuthenticated) {
        return (
            <>
                <button
                    className="btn btn-primary logoutBtn"
                    onClick={() => logout({ })}
                >
                    Log Out
                </button>
                <br />
            </>
        );
    }
}

export default LogoutButton;