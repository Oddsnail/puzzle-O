# Dialogue File Format Reference

Dialogue files (`.txt`) are plain-text scripts that drive the visual novel scene flow.
Each line in a file is read top-to-bottom and classified into one of the following types.

---

## Line Types Overview

| Type | Identifying mark | Description |
|---|---|---|
| **Comment** | Starts with `#` | Ignored at runtime |
| **Empty line** | Blank or whitespace only | Skipped |
| **Label** | Starts with `.` | Jump target; normally skipped unless jumped to |
| **LINE_DIALOGUE** | Starts with `[` | A character speaks a line of text |
| **LINE_COMMAND** | Anything else | One or more commands to execute |

---

## 1. Comment

```
# This is a comment
```

Any line whose first character is `#` is ignored entirely. Use them for section headings or notes.

---

## 2. Label

```
.labelCode
```

A line that starts with `.` defines a jump target. Labels are **skipped** during normal sequential playback. They are only entered when a `j()` or `puzzle()` command redirects execution here.

---

## 3. LINE_DIALOGUE

A dialogue line has two parts: a **speaker header** and a **dialogue body**.

```
[name:spriteCode:speakEvent] "dialogue body"
```

### 3-1. Speaker Header `[name:spriteCode:speakEvent]`

All three fields are separated by `:` and enclosed in `[` `]`.

| Field | Description |
|---|---|
| `name` | Character ID to set as the current speaker. Use `default` for no-name plain text. Leave empty to continue with the most recent speaker. |
| `spriteCode` | Sprite code to apply to the character immediately. Leave empty to keep the current sprite. |
| `speakEvent` | An animation event triggered when the line starts (e.g. `h` = hop, `c` = crouch). Leave empty for no animation. |

**Examples:**

```
[leedoeun::] "..."
```
→ Set speaker to `leedoeun`, no sprite change, no speak event.

```
[:_-_2_-2:h] "?"
```
→ Keep previous speaker, change sprite to `_-_2_-2`, trigger hop animation.

```
[:_-132:c] "뭐, 됐나..."
```
→ Keep previous speaker, change sprite to `_-132`, trigger crouch animation.

```
[default::] "Narrator text here."
```
→ No character name shown, no sprite or speak event.

### 3-2. Dialogue Body

The body follows the header, wrapped in double quotes:

```
"dialogue text here"
```

The text is displayed character-by-character via the text architect. If the text begins with `@`, the rest is treated as a **localization key** and resolved from the current language table at runtime.

```
"@loc_key_001"
```
→ Looks up `loc_key_001` in the localization table and displays the result.

### 3-3. Dialogue Segments

A single dialogue body can be split into multiple **segments** using inline signal tags. Each segment is displayed sequentially within the same dialogue line.

| Signal tag | Name | Behavior |
|---|---|---|
| `{b}` | Break | Wait for player input, then **replace** text with the next segment |
| `{a}` | Append | Wait for player input, then **append** the next segment |
| `{wb:N}` | Wait-Break | Wait `N` seconds, then **replace** text with the next segment |
| `{wa:N}` | Wait-Append | Wait `N` seconds, then **append** the next segment |

`N` is a decimal number representing seconds (e.g. `{wa:0.5}`, `{wb:1.0}`).

**Examples:**

```
[leedoeun::] "음{wa:0.3}.{wa:0.3}.{wa:0.3}."
```
→ Displays `음`, waits 0.3 s, appends `.`, waits 0.3 s, appends `.`, waits 0.3 s, appends `.`.

```
[:_-2_3-_:h] "누군가 쳐다보는 듯한... {a}기분 탓...?"
```
→ Displays `누군가 쳐다보는 듯한... `, waits for input, then appends `기분 탓...?`.

```
[yeonjuhyang:_-122:c] "@loc_test_006{b}@loc_test_007"
```
→ Displays the localized string for `loc_test_006`, waits for input, then replaces it with `loc_test_007`.

After all segments finish, the system waits for one final player input before advancing to the next line.

---

## 4. LINE_COMMAND

Any line that does not start with `[` is parsed as a command line.

### Basic syntax

```
commandName(arg1 arg2 arg3)
```

- Arguments are space-separated inside the parentheses.
- String arguments containing spaces can be wrapped in double quotes: `"my value"`.

### Wait prefix

Prefix a command with `wait-` to force the system to wait for the command to fully complete before advancing to the next line:

```
wait-commandName(args)
```

Some commands **always** wait for completion automatically, even without the prefix (see individual command notes below). The full list of auto-waiting commands is: `wait`, `add`, `choice`, `choiceC`, `puzzle`, `hl`, `uhl`.

### Chaining multiple commands on one line

Use `@` to chain multiple commands on a single line. They execute sequentially:

```
cmd1(args)@cmd2(args)@cmd3(args)
```

---

## 5. Command Reference

### Flow & Timing

---

#### `wait(seconds)`
Pause execution for the given number of seconds.
Always auto-waits.

```
wait(1.0)
wait(0.5)
```

---

#### `j(labelCode)`
Jump to the label `.labelCode`, skipping all lines in between.

```
j(end)
```
→ Skips to `.end`.

---

#### `end(tag)`
End the current conversation and return `tag` to the caller.
The conventional end-of-file tag is `EOF`.

```
end(EOF)
```

---

### Character Management

---

#### `add(name1 name2 ...)`
Add one or more characters to the scene. Always auto-waits.

```
add(leedoeun)
add(leedoeun yeonjuhyang)
```

---

#### `remove(name1 name2 ...)`
Remove one or more characters from the scene.

```
remove(leedoeun)
```

---

### Character Animation

---

#### `appear(name1 name2 ...)`
Play the appear animation for each listed character.
Waits until all listed characters finish appearing.
Use `wait-appear(...)` to also block the next line from running.

```
appear(yeonjuhyang)
wait-appear(leedoeun)
```

---

#### `disappear(name1 name2 ...)`
Play the disappear animation for each listed character.
Waits until all listed characters finish disappearing.

```
disappear(leedoeun yeonjuhyang)
wait-disappear(leedoeun yeonjuhyang)
```

---

#### `setposX(name value [-i bool] [-d seconds])`
Move a character to the given X position.

| Argument | Description |
|---|---|
| `name` | Character ID |
| `value` | Target X position (pixels) |
| `-i bool` | Optional. `true` = move immediately with no animation. Default: `false`. |
| `-d seconds` | Optional. Animation duration. Default: `0.5`. Ignored if `-i true`. |

Waits for animation to finish unless `-i true`.
Use `wait-setposX(...)` to block the next line.

```
setposX(leedoeun -660 -i true)      # teleport immediately
setposX(yeonjuhyang 0 -d 0.1)       # animate in 0.1 s
wait-setposX(leedoeun -550)         # animate (default 0.5 s) and block next line
```

---

#### `setposY(name value [-i bool] [-d seconds])`
Move a character to the given Y position. Same parameters as `setposX`.

```
setposY(yeonjuhyang -260 -i true)
setposY(yeonjuhyang 0 -d 0.1)
```

---

#### `h(name)`
Play the **hop** animation on the character. Waits for completion.

```
h(yeonjuhyang)
```

---

#### `c(name)`
Play the **crouch** animation on the character. Waits for completion.

```
c(leedoeun)
```

---

#### `s(name)`
Play the **shiver** animation on the character. Waits for completion.

```
s(leedoeun)
```

---

#### `sp(name spriteCode)`
Set the character's sprite immediately.

```
sp(leedoeun std1-111)
sp(yeonjuhyang _-111-2)
```

---

#### `ix(name)`
Flip the character horizontally (invert X scale).

```
ix(leedoeun)
```

---

#### `hl(name1 name2 ...)`
Highlight the listed characters (e.g. dim all others).
Always auto-waits until the transition completes.

```
hl(leedoeun)
hl(leedoeun yeonjuhyang)
```

---

#### `uhl(name1 name2 ...)`
Unhighlight (restore) the listed characters.
Always auto-waits until the transition completes.

```
uhl(yeonjuhyang)
```

---

### UI Control

---

#### `closeDialogue()`
Hide the dialogue UI panel.

```
closeDialogue()
```

---

#### `openDialogue()`
Show the dialogue UI panel.

```
openDialogue()
```

---

#### `empty()`
Clear the dialogue text box without hiding the panel.

```
empty()
```

---

### Image / Background

---

#### `changeBackground(imageName)`
Change the background image. The image is loaded by name from the resources.

```
changeBackground(forest)
```

---

#### `changeCutscene(imageName)`
Change the cutscene overlay image.

```
changeCutscene(intro_cutscene)
```

---

#### `quitCutscene()`
Dismiss the cutscene overlay image.

```
quitCutscene()
```

---

### Dynamic Dialogue

---

#### `choice(displayText1 labelCode1 displayText2 labelCode2 ...)`
Display a list of choices. Arguments are **pairs** of `(display text, label code)`.
Always auto-waits until the player selects an option, then jumps to the corresponding label.

```
choice("Option A" labelA "Option B" labelB)
```

---

#### `choiceC(displayText1 labelCode1 color1 displayText2 labelCode2 color2 ...)`
Display a list of choices with per-option color. Arguments are **triplets** of `(display text, label code, color ID)`.
Always auto-waits until the player selects an option, then jumps to the corresponding label.

```
choiceC("무시한다" ig default "쳐다본다" look leedoeun "인사한다" hi yeonjuhyang)
```

---

#### `puzzle(charID difficulty ruleSetCode successLabelCode failLabelCode)`
Start a puzzle minigame. Always auto-waits until the puzzle is resolved.
On success, jumps to `successLabelCode`; on failure, jumps to `failLabelCode`.

| Argument | Description |
|---|---|
| `charID` | Character ID that the puzzle is associated with |
| `difficulty` | Integer difficulty level |
| `ruleSetCode` | The rule set identifier string for the puzzle |
| `successLabelCode` | Label to jump to on success |
| `failLabelCode` | Label to jump to on failure |

The dialogue UI is hidden before the puzzle starts and shown again after it ends.

```
puzzle(leedoeun 7 classic s f)

.s
# success path

.f
# fail path
```

---

## Full Example

```
# initializing

add(leedoeun yeonjuhyang)
ix(leedoeun)
setposX(leedoeun -660 -i true)
setposX(yeonjuhyang 660 -i true)
sp(leedoeun std1-111)
sp(yeonjuhyang std1-112)

# dialogue start

wait-appear(leedoeun)
[leedoeun::] "음{wa:0.3}.{wa:0.3}.{wa:0.3}."
[:_-_2_-2:h] "?"
[:_-132:c] "기분이 이상한데."
closeDialogue()

puzzle(leedoeun 7 classic s f)

.s
openDialogue()
[leedoeun::] "성공!"
j(end)

.f
openDialogue()
[leedoeun::] "실패..."

.end
wait-disappear(leedoeun yeonjuhyang)
end(EOF)
```
