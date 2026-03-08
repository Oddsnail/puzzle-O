using System.Collections;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace origin.command {
    public class CommandManager : MonoBehaviour {
        public static CommandManager instance { get; private set; }
        private static Coroutine process = null;
        public static bool isRunningProcess => process != null;
        private CommandDatabase database;

        private void Awake() {
            if (instance != null && instance != this) {
                Debug.LogWarning($"Duplicate {GetType().Name} detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            instance = this;
            Initialize();
        }

        private void OnDestroy() {
            if (instance == this) {
                instance = null;
            }
        }

        private void Initialize() {
            database = new CommandDatabase();

            try {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] extensionTypes = assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(CMD_DatabaseExtension)) && !t.IsAbstract)
                    .ToArray();

                foreach (Type extension in extensionTypes) {
                    try {
                        MethodInfo extendMethod = extension.GetMethod("Extend", BindingFlags.Public | BindingFlags.Static);

                        if (extendMethod == null) {
                            Debug.LogError($"[ERROR] Extension class {extension.Name} missing static Extend method");
                            continue;
                        }

                        extendMethod.Invoke(null, new object[] { database });
                        Debug.Log($"[INFO] Loaded command extension: {extension.Name}");
                    }
                    catch (Exception ex) {
                        Debug.LogError($"[ERROR] Failed to load extension {extension.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex) {
                Debug.LogError($"[ERROR] Failed to initialize command system: {ex.Message}");
            }
        }

        public Coroutine Execute(string commandName, params string[] args) {

            Delegate command = database.GetCommand(commandName);

            if (command == null) return null;

            return StartProcess(command, args);
        }

        private Coroutine StartProcess(Delegate command, string[] args) {
            StopProcess();
            process = StartCoroutine(RunningProcess(command, args));
            return process;
        }

        public void StopProcess() {
            if (process != null) StopCoroutine(process);
            process = null;
        }

        private IEnumerator RunningProcess(Delegate command, string[] args) {
            yield return WaitProcessComplete(command, args);
            process = null;
        }

        private IEnumerator WaitProcessComplete(Delegate command, string[] args) {
            if (command is Action actionNoArgs) {
                try {
                    actionNoArgs();
                }
                catch (Exception ex) {
                    Debug.LogError($"[ERROR] Command execution failed: {ex.Message}\n{ex.StackTrace}");
                }
            }
            else if (command is Action<string> actionString) {
                if (args.Length < 1) {
                    Debug.LogError($"[ERROR] Command requires 1 argument but got {args.Length}");
                    yield break;
                }
                try {
                    actionString(args[0]);
                }
                catch (Exception ex) {
                    Debug.LogError($"[ERROR] Command execution failed: {ex.Message}\n{ex.StackTrace}");
                }
            }
            else if (command is Action<string[]> actionStringArray) {
                try {
                    actionStringArray(args);
                }
                catch (Exception ex) {
                    Debug.LogError($"[ERROR] Command execution failed: {ex.Message}\n{ex.StackTrace}");
                }
            }
            else if (command is Func<IEnumerator> funcNoArgs) {
                IEnumerator routine = null;
                try {
                    routine = funcNoArgs();
                }
                catch (Exception ex) {
                    Debug.LogError($"[ERROR] Command execution failed: {ex.Message}\n{ex.StackTrace}");
                    yield break;
                }
                yield return routine;
            }
            else if (command is Func<string, IEnumerator> funcString) {
                if (args.Length < 1) {
                    Debug.LogError($"[ERROR] Command requires 1 argument but got {args.Length}");
                    yield break;
                }
                IEnumerator routine = null;
                try {
                    routine = funcString(args[0]);
                }
                catch (Exception ex) {
                    Debug.LogError($"[ERROR] Command execution failed: {ex.Message}\n{ex.StackTrace}");
                    yield break;
                }
                yield return routine;
            }
            else if (command is Func<string[], IEnumerator> funcStringArray) {
                IEnumerator routine = null;
                try {
                    routine = funcStringArray(args);
                }
                catch (Exception ex) {
                    Debug.LogError($"[ERROR] Command execution failed: {ex.Message}\n{ex.StackTrace}");
                    yield break;
                }
                yield return routine;
            }
            else {
                Debug.LogError($"[ERROR] Unsupported command delegate type: {command.GetType()}");
            }
        }
    }
}