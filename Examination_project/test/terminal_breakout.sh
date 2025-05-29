#!/bin/bash

# Tekst og dimensioner
text="${1:-Super, nu har du lavet det 🎉}"
cols=${#text}
rows=12
paddle_width=5
paddle_col=$((cols / 2 - paddle_width / 2))
ball_row=2
ball_col=$((cols / 2))
ball_drow=1
ball_dcol=1

# Terminalforberedelse
stty -echo -icanon time 0 min 0

clear_screen() {
    tput clear
}

draw() {
    clear_screen
    echo "$text"
    for ((r=1; r<rows; r++)); do
        line=""
        for ((c=0; c<cols; c++)); do
            if [[ $r -eq $ball_row && $c -eq $ball_col ]]; then
                line+="o"
            elif [[ $r -eq $((rows - 1)) && $c -ge $paddle_col && $c -lt $((paddle_col + paddle_width)) ]]; then
                line+="="
            else
                line+=" "
            fi
        done
        echo "$line"
    done
}

read_key() {
    if IFS= read -rsn1 -t 0.2 key; then
        if [[ $key == $'\x1b' ]]; then
            IFS= read -rsn1 -t 0.01 k1
            IFS= read -rsn1 -t 0.01 k2
            key+=$k1$k2
        fi
    else
        key=""
    fi
}

update_ball() {
    next_row=$((ball_row + ball_drow))
    next_col=$((ball_col + ball_dcol))

    # Ram væg
    if (( next_col < 0 || next_col >= cols )); then
        ball_dcol=$(( -ball_dcol ))
        next_col=$((ball_col + ball_dcol))
    fi

    # Ram top (tekst)
    if (( next_row == 0 )); then
        if [[ ${text:$next_col:1} != " " ]]; then
            text="${text:0:$next_col} ${text:$((next_col + 1))}"
        fi
        ball_drow=1
        next_row=1
    fi

    # Ram bund (paddle)
    if (( next_row == rows - 1 )); then
        if (( next_col >= paddle_col && next_col < paddle_col + paddle_width )); then
            ball_drow=-1
            next_row=$((rows - 2))
        else
            # Missede paddlen – genstart bolden
            ball_row=2
            ball_col=$((cols / 2))
            ball_drow=1
            ball_dcol=1
            return
        fi
    fi

    ball_row=$next_row
    ball_col=$next_col
}

# Intro
clear_screen
echo "🎮 Terminal Breakout! 🎮"
echo "Styring: ← = venstre, → = højre, q = afslut"
echo "Tryk Enter for at starte..."
read

# Spil-loop
while [[ "$text" =~ [^[:space:]] ]]; do
    read_key
    case "$key" in
        $'\x1b[D') ((paddle_col > 0)) && paddle_col=$((paddle_col - 1)) ;;
        $'\x1b[C') ((paddle_col < cols - paddle_width )) && paddle_col=$((paddle_col + 1)) ;;
        q) break ;;
    esac
    update_ball
    draw
    sleep 0.1
done

draw
echo ""
if [[ "$text" =~ [^[:space:]] ]]; then
    echo "⏹️ Du afsluttede spillet."
else
    echo "🏆 Du har vundet! Hele teksten er væk! 🎉"
fi

# Genskab terminal
stty sane
