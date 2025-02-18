import { Identifier } from "dnd-core";
import { ChangeEvent } from "react";
import { useRef } from "react";
import { useDrag, useDrop, XYCoord } from "react-dnd";

const ItemTypes = {
    ROW: 'ROW',
};

export interface AsksDocumentItemProps {
    id: number;
    index: number;
    points: number;
    description: string;
    moveItem: (dragIndex: number, hoverIndex: number) => void;
    onPointsChanged: (id: number, points: number) => void;
};

interface DragItem {
    index: number;
    id: string;
    type: string;
}

const AsksDocumentItem: React.FC<AsksDocumentItemProps> = ({
    id,
    index,
    points,
    description,
    moveItem,
    onPointsChanged
}) => {
    const ref = useRef<HTMLTableRowElement>(null);

    // Drop hook
    const [{ handlerId }, drop] = useDrop<DragItem, void, { handlerId: Identifier | null }>({
        accept: ItemTypes.ROW,
        collect(monitor) {
            return {
                handlerId: monitor.getHandlerId(),
            };
        },
        hover(item, monitor) {
            if (!ref.current) {
                return;
            }
            const dragIndex = item.index;
            const hoverIndex = index;

            // Don’t do anything if it’s the same row
            if (dragIndex === hoverIndex) {
                return;
            }

            // Figure out rectangle on screen
            const hoverBoundingRect = ref.current.getBoundingClientRect();
            // Get vertical middle
            const hoverMiddleY = (hoverBoundingRect.bottom - hoverBoundingRect.top) / 2;
            // Determine mouse position
            const clientOffset = monitor.getClientOffset();
            if (!clientOffset) return;

            // Get pixels to the top
            const hoverClientY = (clientOffset as XYCoord).y - hoverBoundingRect.top;

            // Only perform the move when the user has crossed half of the item's height
            if (dragIndex < hoverIndex && hoverClientY < hoverMiddleY) {
                return;
            }
            if (dragIndex > hoverIndex && hoverClientY > hoverMiddleY) {
                return;
            }

            // Perform the actual move
            moveItem(dragIndex, hoverIndex);

            // Mutate the item to update its current index for performance
            item.index = hoverIndex;
        },
    });

    // Drag hook
    const [{ isDragging }, drag] = useDrag({
        type: ItemTypes.ROW,
        item: () => {
            return { id, index };
        },
        collect: monitor => ({
            isDragging: monitor.isDragging(),
        }),
    });

    // Combine drag and drop refs
    drag(drop(ref));

    // Handlers
    const handlePointsChange = (e: ChangeEvent<HTMLInputElement>) => {
        const newVal = parseInt(e.target.value, 10) || 0;
        onPointsChanged(id, newVal);
    };

    const opacity = isDragging ? 0 : 1
    return (
        <tr
            style={{ opacity }}
            className="participant-list-item"
            ref={ref}
        >
            <td>
                <input type="number" value={points} onChange={handlePointsChange} />
            </td>
            <td>{description}</td>
        </tr>
    );
};

export default AsksDocumentItem;
