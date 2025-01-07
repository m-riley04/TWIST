import { useAuth0 } from "@auth0/auth0-react";
import { Button } from "react-bootstrap";

const LogoutButton = () => {
    const { logout, isAuthenticated } = useAuth0();

    if (isAuthenticated) {
        return (
            <>
                <Button
                    className="btn btn-primary logoutBtn"
                    onClick={() => logout({ })}
                >
                    Log Out
                </Button>
                <br />
            </>
        );
    }
}

export default LogoutButton;