import React from 'react';
import './App.css';
import { BrowserRouter as Router, Route, Link } from "react-router-dom";
import { Container, Header, Menu, Dropdown, Icon } from "semantic-ui-react"
import HomeView from "./views/home"
import ScriptTerminalView from "./views/script_terminal_view"
import EditorView from "./views/editor_view"
import GridView from "./views/grid_view"
import EventsView from "./views/event_view"
import ComponentsView from "./views/components_view"
import WebSocketComponent from './components/websocket';
import { Provider } from 'react-redux';
import store from './store';



function App() {
  return (
    <div>
      <Provider store={store} >
        <Router>
          <WebSocketComponent />
          <Menu fixed='top' inverted>
            <Container>

              <Menu.Item as='a' header>
                Neon
              </Menu.Item>
              <Menu.Item as='div'>
                <Icon name="home" />
                <Link to='/'>
                  Home
            </Link>
              </Menu.Item>
              <Menu.Item as='div'>
                <Icon name="terminal" />
                <Link to='/script'>
                  Terminal
            </Link>
              </Menu.Item>
              <Menu.Item as='div'>
                <Icon name="write" />
                <Link to='/editor'>
                  Editor
            </Link>
              </Menu.Item>
              <Menu.Item as='a'>
                <Icon name="eye" />
                <Link to='/events'>
                  Events
            </Link>

              </Menu.Item>
              <Menu.Item as='a'>
                <Icon name="grid" />
                <Link to='/grid'>
                  Grid
            </Link>
              </Menu.Item>

            
              <Dropdown pointing className='link item' icon='wrench' text='Settings' >
                  <Dropdown.Menu>
                    <Dropdown.Item>
                      <Link to='/components' >
                         Components
                      </Link>
                    </Dropdown.Item>
                  </Dropdown.Menu>
                </Dropdown>
           
            </Container>
          </Menu>
          <Container style={{ marginTop: '100px' }}>
            <Route path="/" exact component={HomeView} />
            <Route path="/script/" component={ScriptTerminalView} />
            <Route path="/editor/" component={EditorView} />
            <Route path="/events/" component={EventsView} />
            <Route path="/grid/" component={GridView} />
            <Route path="/components/" component={ComponentsView} />
          </Container>
        </Router>
      </Provider>
    </div>


  );
}




export default App;
