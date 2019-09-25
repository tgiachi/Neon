import { EVENT_RECEIVED, ENTITY_UPDATED } from "../constants"

export function onEventReceived(event) {
  return {type: EVENT_RECEIVED, payload: event};
}
export function broadcastData(event) {
  return {type: ENTITY_UPDATED, payload: event};

}