import React from 'react';
import { connect } from 'react-redux';
import { Table } from "semantic-ui-react"
import Moment from 'react-moment';


function mapStateToProps(state) {
  return {
    events: state.events
  }
}

const EventViewer = ({events}) => (
  <Table celled>
    <Table.Header>
      <Table.Row>
        <Table.HeaderCell>Name</Table.HeaderCell>
        <Table.HeaderCell>Group name</Table.HeaderCell>
        <Table.HeaderCell>Entity type</Table.HeaderCell>
        <Table.HeaderCell>Date time</Table.HeaderCell>
      </Table.Row>
    </Table.Header>
    <Table.Body>
      {events.map((event, index) => {
        return (
          <Table.Row key={index}>
            <Table.Cell>{event.data.name}</Table.Cell>
            <Table.Cell>{event.data.group_name}</Table.Cell>
            <Table.Cell>{event.data.entity_type}</Table.Cell>
            <Table.Cell>
              <Moment date={new Date(event.data.event_date_time)} /> 
            </Table.Cell>
          </Table.Row>
        );
      })}
    </Table.Body>
  </Table>
)

export default connect(mapStateToProps)(EventViewer);