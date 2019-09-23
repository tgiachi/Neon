import {
  EVENT_RECEIVED
} from "../constants/"

const initialState  = { events: [] };

function rootReducer(state = initialState, action) {

  if (action.type === EVENT_RECEIVED)
  {
    return Object.assign({}, state, {
      events: state.events.concat(action.payload)
    })
  }
  return state;
}

export default rootReducer;