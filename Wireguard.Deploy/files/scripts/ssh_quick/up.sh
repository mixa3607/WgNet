set -e
IF="$1"

wg-quick up "$IF"
exit 0
