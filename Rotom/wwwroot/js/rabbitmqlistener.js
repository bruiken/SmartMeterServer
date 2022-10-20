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

    error_callback() {
        console.log('An error occured!');
    }

    on_connect(listener) {
        listener.client.subscribe(`/exchange/${listener.exchange}/${listener.locationId}.electricity`, function (d) {
            const content = JSON.parse(d.body);
            const usage_w = (content.kw_usage * 1000).toFixed();
            const generated_w = (content.kw_generated * 1000).toFixed();
            const temp = Date.parse(content.time);

            const data = {
                usage_w,
                generated_w,
                time: temp
            };

            listener.data_callback(data);
        });
    }

    on_error(listener) {
        console.error('An error occured, recreating connection in 10 seconds...');
        (new Promise((r) => setTimeout(r, 10000))).then(() => {
            listener.createClient();
        });
        listener.error_callback();
    }

    createClient() {
        const ws = new WebSocket(`ws://${this.hostname}:${this.port}/ws`);
        this.client = Stomp.over(ws);
        this.client.debug = null;
        this.client.connect(this.username, this.password, () => { this.on_connect(this) }, () => { this.on_error(this) }, this.vhost);
    }
}