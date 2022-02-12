set -e
IF="$1"

wg-quick down "$IF"
exit 0
