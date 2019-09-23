import React from 'react';
import Websocket from 'react-websocket';
import { onEventReceived } from "../actions/"
import {
  connect
} from "react-redux";

import config from "../env"

function mapDispatchToProps(dispatch) {
  return {
    onEventReceived: event => dispatch(onEventReceived(event))
  }
}
class WebSocketComponent extends React.Component{

  render(){
    return (
      < Websocket url = {config.api.WS_URL}
      onMessage = {
        this.handleData.bind(this)
      }
      />      
    )
  }
  handleData(data) {
    let result = JSON.parse(data);
    if (result.messageType !== 2) {
      result.data = JSON.parse(result.data);
       console.log("new event");
       console.log(result);
       this.props.onEventReceived(result);
      }
  }
}

export default  connect(null, mapDispatchToProps)(WebSocketComponent);