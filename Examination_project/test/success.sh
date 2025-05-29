#!/bin/bash
msg="Super, nu har du lavet det 🎉"
for ((i=0; i<${#msg}; i++)); do
    echo -n "${msg:$i:1}"
    sleep 0.05
done
echo ""
