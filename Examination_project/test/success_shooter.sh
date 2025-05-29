#!/bin/bash

cols=$(tput cols)
input="${1:-Super, nu har du lavet det 🎉}"
input="${input:0:$cols}"
rows=20
player_col=$((cols / 2))

# Skud: array af "row,col"
declare -a bullets=()

# Faldende bogstaver
declare -A letters
for ((i = 0; i < cols; i++)); do
    char="${input:$i:1}"
    [[ "$char" != " " ]] && letters["$i,0"]="$char"
done

# Terminal
stty -echo -icanon time 0 min 0

clear_screen() {
    printf "\033[H\033[2J"
}


draw() {
    usleep 10000
    clear_screen
    for ((r = 0; r < rows; r++)); do
        line=""
        for ((c = 0; c < cols; c++)); do
            printed=0
            # Tjek om der er et skud
            for bullet in "${bullets[@]}"; do
                br=${bullet%%,*}
                bc=${bullet##*,}
                if [[ $br -eq $r && $bc -eq $c ]]; then
                    line+="|"
                    printed=1
                    break
                fi
            done

            # Spiller
            if (( printed == 0 )); then
                if [[ $r -eq $((rows - 1)) && $c -eq $player_col ]]; then
                    line+="^"
                elif [[ -n "${letters["$c,$r"]}" ]]; then
                    line+="${letters["$c,$r"]}"
                else
                    line+=" "
                fi
            fi
        done
        echo "$line"
    done
}

fire_bullet() {
    bullets+=("$((rows - 2)),$player_col")
}

update_bullets() {
    new_bullets=()
    for bullet in "${bullets[@]}"; do
        row=${bullet%%,*}
        col=${bullet##*,}
        ((row--))

        if (( row < 0 )); then
            continue
        fi

        hit=0
        for k in "${!letters[@]}"; do
            c=${k%%,*}
            r=${k##*,}
            if (( r == row && c == col )); then
                unset letters["$k"]
                hit=1
                break
            fi
        done

        if (( hit == 0 )); then
            new_bullets+=("$row,$col")
        fi
    done
    bullets=("${new_bullets[@]}")
}

update_letters() {
    local -a active_keys=()
    for k in "${!letters[@]}"; do
        active_keys+=("$k")
    done

    if (( ${#active_keys[@]} == 0 )); then
        return
    fi

    # Vælg tilfældigt bogstav
    idx=$(( RANDOM % ${#active_keys[@]} ))
    key="${active_keys[$idx]}"
    c=${key%%,*}
    r=${key##*,}
    char=${letters["$key"]}
    new_r=$((r + 1))

    # Tjek for game over
    if (( new_r >= rows - 1 )); then
        game_over=1
        return
    fi

    # Flyt bogstav én linje ned
    unset letters["$key"]
    letters["$c,$new_r"]="$char"
}


read_key() {
    if IFS= read -rsn1 -t 0.01 key; then
        if [[ $key == $'\x1b' ]]; then
            IFS= read -rsn1 -t 0.01 k1
            IFS= read -rsn1 -t 0.01 k2
            key+=$k1$k2
        elif [[ $key == $'\x20' ]]; then
            key="space"
        fi
    else
        key=""
    fi
}

# Intro
echo "🎯 SKYD BOGSTAVERNE NED, FØR DE RAMMER DIG!"
echo "← = venstre | → = højre | mellemrum = skyd | q = afslut"
echo "Tryk Enter for at starte..."
read

# Spil-loop
game_over=0
fall_delay=4
fall_counter=0

while (( ${#letters[@]} > 0 && game_over == 0 )); do
    read_key
    case "$key" in
        $'\x1b[D') ((player_col > 0)) && player_col=$((player_col - 1)) ;;
        $'\x1b[C') ((player_col < cols - 1)) && player_col=$((player_col + 1)) ;;
        space) fire_bullet ;;
        q) break ;;
    esac

    update_bullets

    ((fall_counter++))
    if (( fall_counter >= fall_delay )); then
        update_letters
        fall_counter=0
    fi

    draw
    sleep 0.1
done

draw
echo ""
if (( game_over == 1 )); then
    echo "💀 Et bogstav ramte dig... Du tabte!"
elif [[ "$key" == "q" ]]; then
    echo "⏹️ Du afsluttede spillet."
else
    echo "🏆 Du skød alle bogstaverne ned! Du vandt!"
fi

stty sane
