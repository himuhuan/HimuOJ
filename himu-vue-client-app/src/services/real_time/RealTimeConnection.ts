import * as signalR from "@microsoft/signalr";

export default class RealTimeConnection {
	private connection: signalR.HubConnection;

	constructor() {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl("https://localhost:7297/judgehub", {
				skipNegotiation: true,
				transport: signalR.HttpTransportType.WebSockets,
			})
			.withAutomaticReconnect()
			.build();
	}

	async start() {
		await this.connection.start();
	}

	async stop() {
		await this.connection.stop();
	}

	async on(event: string, callback: (...args: any[]) => void) {
		this.connection.on(event, callback);
	}

	async invoke(event: string, ...args: any[]) {
		await this.connection.invoke(event, ...args);
	}

    static async create() {
        const connection = new RealTimeConnection();
        await connection.start();
        return connection;
    }
}
