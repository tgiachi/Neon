import React, { Component, Suspense}from 'react';
import {
  connect
} from 'react-redux';
import GridLayout from 'react-grid-layout';
import '../../node_modules/react-grid-layout/css/styles.css';
import '../../node_modules/react-resizable/css/styles.css';
import WeatherComponent from "../grid_components/weather_component";
import { broadcastData } from "../actions/";
import { Card, Icon, Image, Button } from 'semantic-ui-react'
import LoadGridComponent from './../utils/grid_component_loader'
 
function mapDispatchToProps(dispatch) {
  return {
    broadcastData: event => dispatch(broadcastData(event))
  }
}

class GridView extends Component {

  constructor(props) {
    super(props);
    this.state = { data: [] }
    this.initDownloader();
  }

  initDownloader() {
    setInterval(() => {
      fetch('http://localhost:5000/api/entities/all').then(response => response.json()).then(data => {
        console.log(data);
        this.props.broadcastData(data);
      })
    }, 3000);

  }

  render() {
    var layout = [
      { i: 'a', x: 0, y: 0, w: 1, h: 2, static: true },
      { i: 'b', x: 1, y: 0, w: 3, h: 2, minW: 2, maxW: 4 },
      { i: 'c', x: 4, y: 0, w: 1, h: 2 }
    ];
    return (
      <React.Fragment>
        <Button>Add Entity</Button>
        <GridLayout className="layout" layout={layout} cols={12} rowHeight={30} width={1200}>
          <Card key="a">
            <Card.Description>
              test
          </Card.Description>
          </Card>
          <Card key="b">
            <Card.Description>
            <Suspense fallback="ERROR">
                 <WeatherComponent />
              </Suspense>
            </Card.Description>
          </Card>
          <Card key="c">
            <Card.Description>
              test
          </Card.Description>
          </Card>
        </GridLayout>
      </React.Fragment>

    )
  }
}

export default connect(null, mapDispatchToProps)(GridView)