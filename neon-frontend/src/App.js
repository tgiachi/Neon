import React from 'react';
import './App.css';
import { ThemeProvider, ColorModeProvider, CSSReset } from "@chakra-ui/core";
import { BrowserRouter as Router, Route, Link } from "react-router-dom";
import { Box } from "@chakra-ui/core";
import HomeView  from "./views/home"
import JsEditorComponent from "./components/js_editor";
import WebSocketComponent from './components/websocket';



function App() {
  return (
    <ThemeProvider>
      <CSSReset />
      <ColorModeProvider>
        <Router>
          <WebSocketComponent />
          <div>
            <Box w="90%">
              <JsEditorComponent />
            </Box>
            <Route exact path="/" component={HomeView} />
          </div>
        </Router>
      </ColorModeProvider>
    </ThemeProvider>
  );
}




export default App;
