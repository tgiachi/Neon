import React from 'react';
import MonacoEditor from 'react-monaco-editor';
//https://jsfiddle.net/hec12da1/

class JsEditorComponent extends React.Component {

  constructor(props) {
    super(props);
    this.code = "substr";
  }

  editorDidMount(editor, monaco) {
    console.log('editorDidMount', editor);
    editor.focus();
  }


  render() {

    return (
      <div style={{ width: 100 }}>
        <MonacoEditor
          width="800"
          height="600"
          language="javascript"
          theme="vs-dark"
        />
      </div>

    );

  }
}

export default JsEditorComponent