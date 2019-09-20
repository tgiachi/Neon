import React from 'react';
import * as monaco from 'monaco-editor';
import MonacoEditor from 'react-monaco-editor';

class JsEditorComponent extends React.Component {
  render() {
    return (
      <MonacoEditor
        width="800"
        height="600"
        language="javascript"
        theme="vs-dark"
    />
    )
  }
}

export default JsEditorComponent