import { React, useState, Component } from 'react';
import MonacoEditor from 'react-monaco-editor';
//https://jsfiddle.net/hec12da1/



class JsEditorComponent extends Component {

  constructor(props) {
    super(props);
    this.code = "substr";
    const [completionDisposable, setCompletionDisposable] = useState({});
  }

  
editorWillMount(monaco) {

  this.setCompletionDisposable(
  monaco.languages.registerCompletionItemProvider("javascript", {    // Or any other language...
    provideCompletionItems: (model, position) => {
      return {
        suggestions: [
          [
            {
              label: '"lodash"',
              kind: monaco.languages.CompletionItemKind.Function,
              documentation: "The Lodash library exported as Node.js modules.",
              insertText: '"lodash": "*"'
            },
            {
              label: '"express"',
              kind: monaco.languages.CompletionItemKind.Function,
              documentation: "Fast, unopinionated, minimalist web framework",
              insertText: '"express": "*"'
            },
            {
              label: '"mkdirp"',
              kind: monaco.languages.CompletionItemKind.Function,
              documentation: "Recursively mkdir, like <code>mkdir -p</code>",
              insertText: '"mkdirp": "*"'
            }
          ]
        ]
      };
    }
  }));

}


render() {
  return (
    <MonacoEditor
      width="800"
      height="600"
      language="javascript"
      theme="vs"
      value={this.code}
      editorWillMount={this.editorWillMount}
    />
  )
}
}

export default JsEditorComponent