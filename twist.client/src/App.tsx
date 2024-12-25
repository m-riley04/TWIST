import { BrowserRouter, Route, Routes } from 'react-router-dom';
import './App.css';
import LoginPage from './pages/LoginPage/LoginPage';
import { useAuth0 } from '@auth0/auth0-react';
import LoginButton from './components/LoginButton';

function App() {
    const { isAuthenticated } = useAuth0();

    return (
        <BrowserRouter>
            <Routes>
                {isAuthenticated ? 
                    <Route path="/" element={<LoginPage/>} />
                    : <LoginButton></LoginButton>
                }
            </Routes>
        </BrowserRouter>
    );
}

export default App;