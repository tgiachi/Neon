import React from 'react';
import Terminal from 'terminal-in-react';

class ScriptTerminalComponent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      title: 'test',
      commands: {
        color: {
          method: (args, print, runCommand) => {
            print(`The color is ${args._[0] || args.color}`);
          },
          options: [
            {
              name: 'color',
              description: 'The color the output should be',
              defaultValue: 'white',
            },
          ],
        },
      }}
      this.downloadCommands();

  }


  downloadCommands() {
    fetch("http://localhost:5000/api/scriptengine/functions").then(response => response.json()).then(data => {
      const commands = {};

      data.map((cmd, index) => {
        commands[cmd.function_name] = {
          name: cmd.function_name,
          description: cmd.help_text,
          options: []
        }

        cmd.parameters.map((param, index) => {
          commands[cmd.function_name].options.push({
            name: param.param_name,
            description: param.param_type,
            defaultValue: ''
        })
        })
      });

      this.setState({ title:' ok' ,commands });
      console.log(commands);

    });
  }
  render() {
    console.log(this.state)
    return (
     <div>
        <input type="button" value="OK" onClick={() => this.handleClick()} />
        <Terminal
          color='lime'
          promptSymbol = "#>"
          backgroundColor='black'
          commands={this.state.commands}
          barColor='black'
          style={{ fontWeight: "bold", fontSize: "1em" }}
          msg={this.state.title}
        />
     </div>
    )
  }

  handleClick = () => {
    this.setState({title: 'OK2'});
  }
}

export default ScriptTerminalComponent;