import { useAuth0 } from "@auth0/auth0-react";
import axios from 'axios';

const NotFoundPage = () => {
    return ( 
        <>
            <h1>This page does not exist!</h1>
            <a href="/">Home</a>
        </>
    );
}

export default NotFoundPage;