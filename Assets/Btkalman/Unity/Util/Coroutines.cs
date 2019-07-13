using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Btkalman.Unity.Util {
    public class Coroutines {
        public static float FRAME_SIZE = 0.01f;

        // TODO: Derive this from user settings somehow.
        public static float BUTTON_PRESS_DELAY = 0.2f;

        public static IEnumerator WaitForFrame() {
            yield return new WaitForSeconds(FRAME_SIZE);
        }

        public static IEnumerator WaitForSeconds(float seconds) {
            float start = Time.time;
            while (Time.time - start < seconds) {
                yield return new WaitForSeconds(Math.Min(FRAME_SIZE, Time.time - start));
            }
        }

        public static IEnumerator WaitUntil(Val<bool> done) {
            while (!done.Value) {
                yield return WaitForFrame();
            }
        }

        public static IEnumerator Action(Action action) {
            action();
            yield break;
        }

        public static IEnumerator WaitForButtonDown(string button) {
            if (Input.GetButton(button)) {
                for (var buttonUp = WaitForButtonUp(button); buttonUp.MoveNext();) {
                    yield return buttonUp.Current;
                }
            }
            while (!Input.GetButton(button)) {
                yield return WaitForFrame();
            }
        }

        public static IEnumerator WaitForButtonUp(string button) {
            if (!Input.GetButton(button)) {
                for (var buttonDown = WaitForButtonDown(button); buttonDown.MoveNext();) {
                    yield return buttonDown.Current;
                }
            }
            while (Input.GetButton(button)) {
                yield return WaitForFrame();
            }
        }

        public static IEnumerator WaitForButtonPress(string button, Val<bool> didPress) {
            if (!Input.GetButton(button)) {
                // Button was never down so it wasn't pressed.
                didPress.Value = false;
                yield break;
            }

            float pressSeconds = BUTTON_PRESS_DELAY;
            float start = Time.time;
            while (Time.time - start < pressSeconds) {
                if (!Input.GetButton(button)) {
                    // Button was down at the start but is no longer down within the time frame.
                    didPress.Value = true;
                    yield break;
                }
                yield return new WaitForSeconds(Math.Min(FRAME_SIZE, Time.time - start));
            }

            // Button was not released within the time frame.
            didPress.Value = false;
        }

        public static IEnumerator Race(Val<bool> done, params IEnumerator[] ienums) {
            if (done != null) {
                done.Value = false;
            }
            if (ienums.Length == 0) {
                yield break;
            }
            while (true) {
                foreach (var ienum in ienums) {
                    if (!ienum.MoveNext()) {
                        if (done != null) {
                            done.Value = true;
                        }
                        yield break;
                    }
                    yield return ienum.Current;
                }
            }
        }

        public static IEnumerator Race(params IEnumerator[] ienums) {
            for (var race = Race(null, ienums); race.MoveNext();) {
                yield return race.Current;
            }
        }

        public static IEnumerator Chain(params IEnumerator[] ienums) {
            foreach (var ienum in ienums) {
                while (ienum.MoveNext()) {
                    yield return ienum.Current;
                }
            }
        }
    }
}