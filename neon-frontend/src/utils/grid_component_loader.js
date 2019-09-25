import React from 'react';
import Loadable from 'react-loadable';
import {
  Loader
} from 'semantic-ui-react'
import ReactDynamicImport from "react-dynamic-import";


export default class LoadGridComponent extends React.Component
{

  constructor(props)
  {
    super(props);
    this.state = {
      Component : <Loader />, 
      componentUrl: this.props.component
    }

  }
 
  load() {
    return import(this.props.component)
  }

  componentDidMount(){
    console.log(this.props.component);
    this.setState({
        componentUrl: this.props.component
    })
  }

  render() {
    console.log('RENDER')
    const loader = () => import(this.props.component)
    
   const RealComponent = ReactDynamicImport({loader});

    return <RealComponent />
  }
}