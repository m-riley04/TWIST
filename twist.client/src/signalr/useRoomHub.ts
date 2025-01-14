import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { useState, useEffect } from "react";
import { SERVER_URL } from "../server/server_consts";

export function useRoomHub(simCode: string) {
    const [connection, setConnection] = useState<HubConnection>();

    useEffect(() => {
        const conn = new HubConnectionBuilder()
            .withUrl(`${SERVER_URL}/roomHub`)
            .withAutomaticReconnect()
            .build();
    
            conn.start()
                .catch(console.error);
            setConnection(conn);

        return () => {
            conn.stop().catch(console.error);
        };
    }, [simCode]);

    return connection;
}
