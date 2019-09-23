import React from 'react';
import './App.css';
import { BrowserRouter as Router, Route, Link } from "react-router-dom";
import { Container, Header, Menu, Dropdown, Icon } from "semantic-ui-react"
import HomeView from "./views/home"
import ScriptTerminalView from "./views/script_terminal_view"
import EditorView from "./views/editor_view"
import EventsView from "./views/event_view"
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
              <Menu.Item as='a'>
                <Icon name="home" />
                <Link to='/'>
                  Home
            </Link>
              </Menu.Item>
              <Menu.Item as='a'>
                <Icon name="terminal" />
                <Link to='/script'>
                  Terminal
            </Link>
              </Menu.Item>
              <Menu.Item as='a'>
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

            </Container>
          </Menu>
          <Container style={{marginTop: '100px'}}>
            <Route path="/" exact component={HomeView} />
            <Route path="/script/" component={ScriptTerminalView} />
            <Route path="/editor/" component={EditorView} />
            <Route path="/events/" component={EventsView} />
          </Container>
        </Router>
      </Provider>
    </div>


  );
}




export default App;
