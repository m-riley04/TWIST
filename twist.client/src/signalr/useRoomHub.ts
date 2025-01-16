import { HubConnection, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import { useState, useEffect } from "react";
import { SERVER_URL } from "../server/server_consts";

export function useRoomHub(simCode: string) {
    const [connection, setConnection] = useState<HubConnection>();

    useEffect(() => {
        const conn = new HubConnectionBuilder()
            .withUrl(`${SERVER_URL}/room-hub`)
            .withAutomaticReconnect()
            .build();

        setConnection(conn);

        return () => {
            // Don't stop here
        };
    }, [simCode]);

    useEffect(() => {
        if (!connection) return;

        if (connection.state !== HubConnectionState.Disconnected) {
            console.error("Cannot start connection when it is not in the Disconnected state.");
            return;
        }

        let didCancel = false;

        connection.start()
            .then(() => {
                if (!didCancel) {
                    console.log("Connection started.");
                }
            })
            .catch(console.error);

        return () => {
            didCancel = true;
            connection.stop().catch(console.error);
        };
    }, [connection]);


    return connection;
}
