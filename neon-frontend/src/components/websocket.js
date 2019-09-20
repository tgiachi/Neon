import React from 'react';
import Websocket from 'react-websocket';
import { List, ListItem, ListIcon } from "@chakra-ui/core";
class WebSocketComponent extends React.Component{

  constructor(props){
    super(props);
    this.state = { messages: []}
  }
  render(){
    return (
      <div>
      <Websocket url="ws://localhost:5000/ws/events" onMessage={this.handleData.bind(this)} />
        <List styleType="disc">
        {this.state.messages.map((value, index) => {
          return <ListItem key={index}><code>{value.data}</code> </ListItem>
        })}
        </List>
      </div>
    )
  }
  handleData(data) {
    let result = JSON.parse(data);
    if (result.messageType !== 2)
      this.setState({ messages:  [...this.state.messages ,result] })
    console.log(result);
  }
}

export default WebSocketComponent;