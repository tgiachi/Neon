import React from 'react';
import {
  connect
} from 'react-redux';
import Skycons from 'react-skycons'

function mapStateToProps(state) {
  return {
    data: state.data
  }
}
class WeatherComponent extends React.Component {

  constructor(props)
  {
    super(props);
    this.state = {data: {}};
  }

  componentDidUpdate(prevProps, props)
  { 
    if (prevProps.data !== this.props.data)
    {
      const weatherData = this.props.data.filter((value) => {
        return value.GroupName === 'WEATHER'}
        );
      if (weatherData !== undefined){
        const weather = weatherData[0];
        weather.Icon = weather.Icon.split(/(?=[A-Z])/).join('_').toUpperCase();       
        this.setState({
          data: weather
        });
      }
    }
  }

  render() {
    return (
      <React.Fragment>
         <div>Temperature: {this.state.data.Temperature}</div>
         <div>Humidity: {this.state.data.Humidity}%</div>
          <div>Summary: {this.state.data.Summary}</div>
         
         <div><Skycons Icon={this.state.data.Icon} autoplay={true} /></div>

      </React.Fragment>
     
    )
  }
}

export default connect(mapStateToProps)(WeatherComponent)