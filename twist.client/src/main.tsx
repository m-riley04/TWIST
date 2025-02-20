import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { Auth0Provider } from '@auth0/auth0-react'
import App from './App.tsx'
import './index.scss'
import { DndProvider } from 'react-dnd'
import { HTML5Backend } from 'react-dnd-html5-backend';

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <Auth0Provider
            domain={"juqbox.us.auth0.com"}
            clientId={import.meta.env.VITE_AUTH0_CLIENT_ID}
            authorizationParams={{
                redirect_uri: "https://twist-server.azurewebsites.net" + "/instructor",

            }}
            cacheLocation="localstorage"
            useRefreshTokens={true}
        >
            <DndProvider backend={HTML5Backend}>
                <App />
            </DndProvider>
        </Auth0Provider>
    </StrictMode>,
)
