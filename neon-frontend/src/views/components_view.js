
import React from 'react';
import {
 
 Table, Button
} from 'semantic-ui-react'
export default class ComponentsView extends React.Component
{ 
  constructor(props){
    super(props);
    this.state = {availableComponents: []}
  }

  componentDidMount(){
    fetch('http://localhost:5000/api/components/available').then(data => data.json()).then(result => this.setState({availableComponents: result}));
  }

  render(){
    return (
      <Table celled >
        <Table.Header>
          <Table.Row>
            <Table.HeaderCell>Name</Table.HeaderCell>
            <Table.HeaderCell>Category</Table.HeaderCell>
            <Table.HeaderCell>Description</Table.HeaderCell>
            <Table.HeaderCell></Table.HeaderCell>
          </Table.Row>
        </Table.Header>
        <Table.Body>
          {this.state.availableComponents.map((component, index) =>{
            return (
                <Table.Row key={index}>
                  <Table.Cell>{component.name}</Table.Cell>
                  <Table.Cell>{component.category}</Table.Cell>
                  <Table.Cell>{component.description}</Table.Cell>
                  <Table.Cell>
                      <Button>Start</Button>
                  </Table.Cell>
                </Table.Row>
            )
          })}
        </Table.Body>
      </Table>
    )
  }
}