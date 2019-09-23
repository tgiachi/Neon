import React from 'react';
import ReactTerminal from 'react-terminal-component';
import {
  EmulatorState,
  CommandMapping,
  OutputFactory
} from 'javascript-terminal';

import config from "../env"

class ScriptTerminalComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      title: 'Neon',
      commands: {},
      emuState: EmulatorState.create({
         'commandMapping': CommandMapping.create({
           print: {
             'function': (state, opts) => {
               const input = opts.join(' ');

               return {
                 output: OutputFactory.makeTextOutput(input)
               };
             },
             'optDef': {}
           }
         })
      })}
      console.log(this.state);
  }

  async componentDidMount()
  {
    await this.test();
  }

  async test() {
      const response = await fetch(`${config.api.API_URL}api/scriptengine/functions`);
      const data = await response.json();
      const commands = {};
      data.map((cmd, index) => {

        commands[cmd.function_name] = {
          'function': (state, opts) => {
            console.log(state)
            this.sendCommand(cmd.function_name, (text) => {
              console.log(text);
            })
           return {
             output: OutputFactory.makeTextOutput(cmd.function_name)
           }; 
          },
           'optDef': {}
        }
      });
      console.log(`commands ${commands}`)
      console.log(commands);
      this.setState({
         emuState: EmulatorState.create({
               'commandMapping': CommandMapping.create(
               commands
               )
             }
      )}); 
      
  }


  sendCommand(cmd, print)
  {
    console.log(cmd);
    print('sending command ' + cmd);
    fetch(`${config.api.API_URL}api/scriptengine/execute/script?script=${encodeURIComponent(cmd)}`, {
      method: 'POST',
      headers: 
      { 'Content-Type': 'application/json'
    }
      }).then(data => data.json()
      ).then(result => {
        if (result.is_error)
        {
          print(`Error: ${result.error_message}`)
        }
        else{
          print(`> ${result.result}`)
        }
      console.log(result);
    })
  }
  render() {
    console.log(this.state)
    return (
     <div>
       <ReactTerminal emulatorState={this.state.emuState} />
     </div>
    )
  }

}

export default ScriptTerminalComponent;