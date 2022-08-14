import React, { Component } from 'react';

import InputField from './InputField/InputField';

import add from '../images/add-circle.png';
import remove from '../images/delete.png';

import { sendJSONRequest } from '../requestFuncs';

import './MembersSection.css';

class MembersSection extends Component {
    state = {
        currentMemberId: "",
        currentMember: null,
        members: [],
    }

    getMembers() {
        return this.state.members.map(item => item.id);
    }

    handleMemberInput = (event) => {
        this.setState({ currentMemberId: event.target.value });

        sendJSONRequest("GET", `/user/get_minimal/${event.target.value}`, undefined, this.props.tokens.token)
            .then(response => {
                console.log(response);
                this.setState({ currentMember: response.data });
            }, error => {
                console.log(error);
                this.setState({ currentMember: null });
            });
    }

    handleAdd = () => {
        if (this.state.currentMember && this.state.members.map(item => item.id).indexOf(this.state.currentMember.id) ===  -1) {
            this.setState({
                members: [...this.state.members, this.state.currentMember],
                currentMemberId: "",
                currentMember: null
            });

        }
    }

    handle = (index) => {
        
    }

    renderPreviewBox = () => {
        if (this.state.currentMember) {
            return <InputField.IconTwoLineItem icon={this.state.currentMember.imageUrl} header={this.state.currentMember.userName}
                text={this.state.currentMember.email} />
        }
        else if (this.state.currentMemberId !== "") {
            console.log(this.state.currentMemberId);
            return <p className="members-no-result-text">No users found</p>
        }
    }

    render() {
        return (
            <div className="members-section-container fill">
                <p className="members-section-header">Members</p>
                <div className="flex">
                    <div className="fill">
                        <InputField design="m2" width="95%" showUnderline={true} value={this.state.currentMemberId} width={this.props.inputWidth} onChange={this.handleMemberInput}>
                            <div className="members-section-preview-box" style={{
                                border: this.state.currentMemberId !== "" ? "1px solid black" : "none",
                                borderTop: "none",
                                width: this.props.inputWidth
                            }}>
                                {
                                    this.renderPreviewBox()
                                }
                            </div>
                            {
                                this.state.members.map((item, index) =>
                                    <div key={index} className="members-section-member">
                                        <InputField.IconTwoLineItem key={index} icon={item.imageUrl} header={item.userName} text={item.email} />
                                        <img src={remove} alt="" height={20} width={20}
                                            onClick={() => this.setState({
                                                members: this.state.members.filter((_, item_index) => item_index !== index)
                                            })} />
                                    </div>
                                )
                            }
                        </InputField>
                    </div>
                    <img src={add} alt="" height={20} width={20} onClick={this.handleAdd} />
                </div>
            </div>    
        )
    }
}

export default MembersSection;