//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Multiple Notes Sheet
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: August 07, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script simply extends the NotesMovement script to add functionality
//		to transition to another note if This note has gone out of the specific
//		boundaries. Rather than lerp back to a valid position, it will reveal
//		another note instead.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class MultipleNotesSheet : NotesMovement
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public MultipleNotesManager m_rNotesManager;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool m_bCanSwitchNotes = true;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool CanSwitchNotes { get { return m_bCanSwitchNotes; } set { m_bCanSwitchNotes = value; } }

	protected override bool CanMoveX { get { return true; } }
	


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Check Limit
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void CheckLimit()
	{
		if(m_rNotesManager != null && CanSwitchNotes)
		{
			if (!IsValidScale(LocalScale))
			{
				CorrectScaleSize();
				return;
			}

			// Transition to Prev/Next note if Note has gone out of Bounds
			OutOfBoundsIdentity eBoundary = GetOutOfBounds();
			if (eBoundary != OutOfBoundsIdentity.NOT_OUT_OF_BOUNDS)
			{
				if (eBoundary == OutOfBoundsIdentity.EAST && m_rNotesManager.HasPreviousNote && LocalPosition.x > EasternBoundary * 1.25f)
				{
					m_rNotesManager.Disappear( MultipleNotesManager.TransitioningNote.PREVIOUS_NOTE );
				}
				else if (eBoundary == OutOfBoundsIdentity.WEST && m_rNotesManager.HasNextNote && LocalPosition.x < WesternBoundary * 1.25f)
				{
					m_rNotesManager.Disappear( MultipleNotesManager.TransitioningNote.NEXT_NOTE );
				}
				else
				{
					CorrectMovementPosition();
				}
			}
		}
		else
		{
			base.CheckLimit();
		}
	}
}
