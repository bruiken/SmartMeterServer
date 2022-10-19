class RabbitMQListener {
    client = null;

    constructor(hostname, port, username, password, vhost, exchange, locationId) {
        this.hostname = hostname;
        this.port = port;
        this.username = username;
        this.password = password;
        this.vhost = vhost;
        this.exchange = exchange;
        this.locationId = locationId;
        this.password = password;
        this.createClient();
    }

    data_callback(data) {
        console.log(data);
    }

    on_connect(listener) {
        listener.client.subscribe(`/exchange/${listener.exchange}/${listener.locationId}.electricity`, function (d) {
            let content = JSON.parse(d.body);
            let usage_w = (content.kw_usage * 1000).toFixed();
            let generated_w = (content.kw_generated * 1000).toFixed();

            let data = {
                usage_w,
                generated_w
            };

            listener.data_callback(data);
        });
    }

    on_error(listener) {
        console.error('An error occured, recreating connection in 10 seconds...');
        (new Promise((r) => setTimeout(r, 10000))).then(() => {
            listener.createClient();
        });
    }

    createClient() {
        let ws = new WebSocket(`ws://${this.hostname}:${this.port}/ws`);
        this.client = Stomp.over(ws);
        this.client.debug = null;
        this.client.connect(this.username, this.password, () => { this.on_connect(this) }, () => { this.on_error(this) }, this.vhost);
    }
}