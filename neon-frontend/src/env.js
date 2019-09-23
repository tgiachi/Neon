const dev = {
  api : {
    API_URL: "http://localhost:5000/",
    WS_URL: 'ws://localhost:5000/ws/events'
  }
}

const prod = {
  api: {
    API_URL: "/",
    WS_URL: 'ws:///ws/events'
  }
}

const config = process.env.REACT_APP_STAGE === 'production' ?
  prod :
  dev;


  export default {
    MAX_ATTACHMENT_SIZE: 5000000,
    ...config
  };

