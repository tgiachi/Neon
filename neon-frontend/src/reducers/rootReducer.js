import {
  EVENT_RECEIVED, ENTITY_UPDATED
} from "../constants/"

const initialState  = { events: [], data:[] };

function rootReducer(state = initialState, action) {

  if (action.type === EVENT_RECEIVED)
  {
    return Object.assign({}, state, {
      events: state.events.concat(action.payload)
    })
  }
  if (action.type === ENTITY_UPDATED)
  {
    return { data: action.payload}
  }
  return state;
}

export default rootReducer;