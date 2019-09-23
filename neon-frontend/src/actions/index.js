import { EVENT_RECEIVED } from "../constants"

export function onEventReceived(event) {
  return {type: EVENT_RECEIVED, payload: event};
}